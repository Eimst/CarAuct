import React from 'react';
import EmptyFilter from "@/app/components/EmptyFilter";

interface PageProps {
    searchParams: Promise<{ callbackUrl?: string }>;
}

const SignIn = async ({ searchParams }: PageProps) => {
    const resolvedParams = await searchParams;

    return (
        <div>
            <EmptyFilter
                title={`You need to be logged in to do that`}
                subtitle={`Click below to login`}
                showLogin={true}
                callbackUrl={resolvedParams.callbackUrl}
            />
        </div>
    );
};

export default SignIn;