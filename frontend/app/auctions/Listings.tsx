'use client'

import React, {useEffect, useState} from 'react';
import AuctionCard from "@/app/auctions/AuctionCard";
import AppPagination from "@/app/components/AppPagination";
import {getData} from "@/app/actions/auctionAction";
import {Auction, PageResult} from "@/types";
import Filters from "@/app/auctions/Filters";
import {useParamStore} from "@/hooks/useParamsStore";
import {useShallow} from "zustand/react/shallow";
import qs from 'query-string';
import EmptyFilter from "@/app/components/EmptyFilter";


const Listings = () => {
    const [data, setData] = useState<PageResult<Auction>>();

    const params = useParamStore(useShallow(state => ({
        pageNumber: state.pageNumber,
        pageSize: state.pageSize,
        searchTerm: state.searchTerm,
        orderBy: state.orderBy,
        filterBy: state.filterBy,
    })))

    const setParams = useParamStore(state => state.setParams);
    const url = qs.stringifyUrl({url: '', query: params})

    const setPageNumber = (pageNumber: number) => {
        setParams({pageNumber});
    }

    useEffect(() => {
        getData(url).then(data => {
            setData(data);
        })
    }, [url]);

    if (!data) {
        return <h3>Loading...</h3>
    }

    return (
        <>
            <Filters/>
            {data.totalCount === 0 ? (
                <EmptyFilter showReset={true}></EmptyFilter>
            ) : (
                <>
                    <div className="grid grid-cols-4 gap-6">
                        {data.results.map((auction) => (
                            <AuctionCard auction={auction} key={auction.id}></AuctionCard>
                        ))}
                    </div>
                    <div className="flex justify-center mt-4">
                        <AppPagination currentPage={params.pageNumber} pageCount={data.pageCount}
                                       pageChanged={setPageNumber}/>
                    </div>
                </>
            )}
        </>
    );
};

export default Listings;