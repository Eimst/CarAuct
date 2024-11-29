'use client'

import React from 'react';
import {FaSearch} from "react-icons/fa";
import {useParamStore} from "@/hooks/useParamsStore";
import {usePathname, useRouter} from "next/navigation";

const Search = () => {
    const setParams = useParamStore(state => state.setParams)
    const setSearchValue = useParamStore(state => state.setSearchValue)
    const searchValue = useParamStore(state => state.searchValue)
    const router = useRouter()
    const pathName = usePathname()

    const onChange = (event: any) => {
        setSearchValue(event.target.value);
    }

    const search = () => {
        if (pathName !== '/')
            router.push('/')
        setParams({searchTerm: searchValue});
    }

    return (
        <div className='flex w-[50%] items-center border-2 rounded-full py-2 shadow-sm'>
            <input
                onKeyDown={(e: any) => {
                    if (e.key === 'Enter') {
                        search()
                    }
                }}
                value={searchValue}
                onChange={onChange}
                type="text"
                placeholder='Search for cars'
                className='flex-grow pl-5 bg-transparent focus:outline-none border-transparent focus:border-transparent
                   focus:ring-0 text-sm text-gray-600'/>
            <button
                onClick={search}
            >
                <FaSearch
                    size={34}
                    className='bg-red-400 text-white rounded-full p-2 cursor-pointer mx-2'
                />
            </button>
        </div>
    );
};

export default Search;