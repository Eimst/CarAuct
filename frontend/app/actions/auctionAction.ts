'use server'

import {Auction, Bid, PageResult} from "@/types";
import {fetchWrapper} from "@/app/lib/fetchWrapper";
import {FieldValues} from "react-hook-form";
import {revalidatePath} from "next/cache";

export const getData: (query: string) => Promise<PageResult<Auction>> = async (query: string) => {
    return await fetchWrapper.get('search' + query);
}

export const updateAuctionTest = async () => {
    const data = {
        mileage: Math.floor(Math.random() * 10000) + 1,
    }

    return await fetchWrapper.put('auctions/afbee524-5972-4075-8800-7d1f9d7b0a0c', data)
}

export async  function createAuction(data: FieldValues) {
    return await fetchWrapper.post('auctions', data);
}

export async function getDetailViewData(id: string) : Promise<Auction> {
    return await fetchWrapper.get(`auctions/${id}`);
}

export async function updateAuction(data: FieldValues, id: string) {
    const res = await fetchWrapper.put(`auctions/${id}`, data)
    revalidatePath(`/auctions/${id}`)
    return res;
}

export async function deleteAuction(id: string) {
    return await fetchWrapper.del(`auctions/${id}`);
}

export async function getBidsForAuction(id: string): Promise<Bid[]> {
    return await fetchWrapper.get(`bids/${id}`);
}

export async function placeBidForAuction(id: string, amount: number) {
    return await fetchWrapper.post(`bids?auctionId=${id}&amount=${amount}`, {})
}