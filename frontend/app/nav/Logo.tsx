'use client'

import React from 'react';
import {AiOutlineCar} from "react-icons/ai";
import {useParamStore} from "@/hooks/useParamsStore";
import {usePathname, useRouter} from "next/navigation";

const Logo = () => {
    const reset = useParamStore(state => state.reset);
    const router = useRouter();
    const pathName = usePathname()

    const doReset = () => {
        if (pathName !== "/") {
            router.push("/");
        }
        reset();
    }

    return (
        <div onClick={doReset} className="cursor-pointer flex items-center gap-2 text-3xl font-semibold text-red-500">
            <AiOutlineCar size={34}></AiOutlineCar>
            <div>
                Car Auctions
            </div>
        </div>
    )
};

export default Logo;