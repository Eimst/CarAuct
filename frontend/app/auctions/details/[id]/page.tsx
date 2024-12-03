import React from 'react';
import {getDetailViewData} from "@/app/actions/auctionAction";
import Heading from "@/app/components/Heading";
import CountdownTimer from "@/app/auctions/CountdownTimer";
import CardImage from "@/app/auctions/CardImage";
import DetailedSpecs from "@/app/auctions/details/[id]/DetailedSpecs";
import {getCurrentUser} from "@/app/actions/authActions";
import EditButton from "@/app/auctions/details/[id]/EditButton";
import DeleteButton from "@/app/auctions/details/[id]/DeleteButton";
import BidList from "@/app/auctions/details/[id]/BidList";


interface PageProps {
    params: Promise<{ id: string }>;
}

const Details = async ({ params }: PageProps) => {
    const resolvedParams = await params;
    const data = await getDetailViewData(resolvedParams.id);
    const user = await getCurrentUser();

    return (
        <div className={`shadow-2xl p-5`}>
            <div className={`flex justify-between`}>
                <div className={`flex items-center gap-6`}>
                    <Heading title={`${data.make} ${data.model}`}/>
                    {user?.username === data.seller && (
                        <>
                            <EditButton id={data.id}/>
                            <DeleteButton id={data.id}/>
                        </>
                    )}

                </div>

                <div className={`flex gap-3`}>
                    <h3 className={`text-2xl font-semibold`}>Time remaining:</h3>
                    <CountdownTimer auctionEnd={data.auctionEnd}/>
                </div>

            </div>
            <div className={`grid grid-cols-2 gap-6 mt-3`}>
                <div className={`w-full bg-gray-200 relative aspect-[4/3] rounded-lg overflow-hidden`}>
                    <CardImage imageUrl={data.imageUrl}/>
                </div>

                <BidList user={user} auction={data}/>
            </div>
            <div className={`mt-4 grid grid-cols-1 rounded-lg`}>
                <DetailedSpecs auction={data}/>
            </div>
        </div>
    );
};

export default Details;