'use client'

import React, {useState} from 'react';
import {Button} from "flowbite-react";
import {useRouter} from "next/navigation";
import {deleteAuction} from "@/app/actions/auctionAction";
import toast from "react-hot-toast";

type Props = {
    id: string
}

function DeleteButton({id}: Props) {
    const [loading, setLoading] = useState(false);
    const router = useRouter();

    const doDelete = () => {
        setLoading(true);
        deleteAuction(id).then((res) => {

            if (res.error) {
                throw res;
            }

            router.push("/")

            // eslint-disable-next-line @typescript-eslint/no-explicit-any
        }).catch((error: any) => {
            toast.error(`${error.status} ${error.message}`);
        }).finally(() => setLoading(false));

    }

    return (
        <div>
            <Button
                isProcessing={loading}
                onClick={doDelete}
                color={'failure'}
                >
                Delete Auction
            </Button>
        </div>
    );
}

export default DeleteButton;