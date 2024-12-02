import React from 'react';
import CountdownTimer from "@/app/auctions/CountdownTimer";
import CardImage from "@/app/auctions/CardImage";
import {Auction} from "@/types";
import Link from "next/link";
import CurrentBid from "@/app/auctions/currentBid";


type Props = {
    auction: Auction
}

const AuctionCard = ({auction}: Props) => {
    return (
        <Link href={`/auctions/details/${auction.id}`} className="group">
            <div className="relative w-full bg-gray-200 aspect-[16/10] rounded-lg overflow-hidden">
                <CardImage imageUrl={auction.imageUrl}></CardImage>
                <div className="absolute bottom-2 left-2">
                    <CountdownTimer auctionEnd={auction.auctionEnd}></CountdownTimer>
                </div>
                <div className="absolute top-2 right-2">
                    <CurrentBid reservePrice={auction.reservePrice} amount={auction.currentHigh}/>
                </div>
            </div>
            <div className="flex justify-between items-center mt-4">
                <h3 className="text-gray-700">{auction.make} {auction.model}</h3>
                <p className="font-semibold text-sm">{auction.year}</p>
            </div>
        </Link>
    );
};

export default AuctionCard;