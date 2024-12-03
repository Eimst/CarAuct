'use client'

import React, {ReactNode, useCallback, useEffect, useRef} from 'react';
import {HubConnection, HubConnectionBuilder} from "@microsoft/signalr";
import {useAuctionStore} from "@/hooks/useAuctionStore";
import {useBidStore} from "@/hooks/useBidStore";
import {useParams} from "next/navigation";
import {Auction, AuctionFinished, Bid} from "@/types";
import {User} from "next-auth";
import toast from "react-hot-toast";
import AuctionCreatedToast from "@/app/components/AuctionCreatedToast";
import {getDetailViewData} from "@/app/actions/auctionAction";
import AuctionFinishedToast from "@/app/components/AuctionFinishedToast";

type Props = {
    children: ReactNode
    user: User | null
    notifyUrl: string
}

function SignalRProvider({children, user, notifyUrl}: Props) {
    const connection = useRef<HubConnection | null>(null);
    const setCurrentPrice = useAuctionStore(state => state.setCurrentPrice);
    const addBid = useBidStore(state => state.addBid);
    const params = useParams<{ id: string }>();

    const handleAuctionFinished = useCallback((finishedAuction: AuctionFinished) => {
        const auction = getDetailViewData(finishedAuction.auctionId)
        return toast.promise(auction, {
            loading: 'Loading',
            success: (auction) =>
                <AuctionFinishedToast
                    finishedAuction={finishedAuction}
                    auction={auction}
                />,
            error: () => 'Auction finished'
        }, {success: {duration: 10000, icon: null}})
    }, [])

    const handleAuctionCreated = useCallback((auction: Auction) => {
        if (user?.username !== auction.seller) {
            return toast(<AuctionCreatedToast auction={auction}/>, {
                duration: 10000
            })
        }
    }, [user?.username]);

    const handleBidPlaced = useCallback((bid: Bid) => {
        if (bid.bidStatus.includes('Accepted')) {
            setCurrentPrice(bid.auctionId, bid.amount)
        }

        if (params.id === bid.auctionId) {
            addBid(bid)
        }
    }, [setCurrentPrice, addBid, params.id])

    useEffect(() => {
        if (!connection.current) {
            connection.current = new HubConnectionBuilder()
                .withUrl(notifyUrl)
                .withAutomaticReconnect()
                .build();

            connection.current.start()
                .then(() => 'Connected to notificationHub')
                .catch(console.error);
        }
        connection.current.on('BidPlaced', handleBidPlaced);
        connection.current.on('AuctionCreated', handleAuctionCreated);
        connection.current.on('AuctionFinished', handleAuctionFinished);

        return () => {
            connection.current?.off('BidPlaced', handleBidPlaced);
            connection.current?.off('AuctionCreated', handleAuctionCreated);
            connection.current?.off('AuctionFinished', handleAuctionFinished);
        }
    }, [setCurrentPrice, handleBidPlaced, handleAuctionCreated, handleAuctionFinished, notifyUrl]);

    return (
        children
    );
}

export default SignalRProvider;