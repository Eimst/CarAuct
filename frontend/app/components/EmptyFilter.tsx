'use client'

import React from 'react';
import {useParamStore} from "@/hooks/useParamsStore";
import Heading from "@/app/components/Heading";
import {Button} from "flowbite-react";
import {signIn} from "next-auth/react";


type Props = {
    title?: string;
    subtitle?: string;
    showReset?: boolean;
    showLogin?: boolean;
    callbackUrl?: string;
}

const EmptyFilter = (
    {
        title = 'No matches for this filter',
        subtitle = 'Try changing the filter',
        showReset,
        showLogin,
        callbackUrl
    } : Props) => {

    const reset = useParamStore(state => state.reset)

    return (
        <div className='h-[40vh] flex flex-col  gap-2 items-center justify-center shadow-lg'>
            <Heading title={title} subtitle={subtitle} center={true}/>
            <div className={`mt-4`}>
                {showReset && (
                    <Button
                        outline
                        className={`rounded-2xl border-2 border-red-400 focus:ring-1 focus:ring-red-400`}
                        onClick={reset}
                    >
                        Remove filters
                    </Button>
                )}
                {showLogin && (
                    <Button
                        outline
                        className={`rounded-2xl border-2 border-red-400 focus:ring-1 focus:ring-red-400`}
                        onClick={() => signIn('id-server', {redirectTo: callbackUrl})}
                    >
                        Login
                    </Button>
                )}
            </div>
        </div>
    );
};

export default EmptyFilter;