import React from 'react';
import {Auction} from "@/types";
import Link from "next/link";
import Image from "next/image";


type Props = {
    auction: Auction;
}

function AuctionCreatedToast({auction}: Props) {
    return (
        <div>
            <Link href={`/auctions/details/${auction.id}`} className={`flex flex-col items-center`}>
                <div className={`flex flex-row items-center gap-2`}>
                    <Image
                        src={auction.imageUrl}
                        alt={`image of car`}
                        height={80}
                        width={80}
                        className={`rounded0lg w-auto h-auto`}
                    />
                    <span>
                        New Auction: {auction.make} {auction.model} has been added
                    </span>
                </div>
            </Link>
        </div>
    );
}

export default AuctionCreatedToast;