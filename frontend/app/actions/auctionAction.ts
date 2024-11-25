'use server'

import {Auction, PageResult} from "@/types";

export const getData: (query: string) => Promise<PageResult<Auction>> = async (query: string) => {
    const res = await fetch(`http://localhost:6001/search${query}`);
    if (!res.ok) {
        throw new Error('Failed to fetch data');
    }
    return res.json();
}