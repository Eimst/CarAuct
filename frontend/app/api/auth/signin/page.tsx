import React from 'react';
import EmptyFilter from "@/app/components/EmptyFilter";

const SignIn = ({searchParams} : {searchParams: {callbackUrl: string}}) => {
    return (
        <div>
            <EmptyFilter
                title={`You need to be logged in to do that`}
                subtitle={`Click below to login`}
                showLogin={true}
                callbackUrl={searchParams.callbackUrl}
            />
        </div>
    );
};

export default SignIn;