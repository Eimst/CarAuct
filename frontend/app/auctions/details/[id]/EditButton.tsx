'use client'

import React from 'react';
import {Button} from "flowbite-react";
import Link from "next/link";

type Props = {
    id: string
}

function EditButton({id}: Props) {
    return (
        <div>
            <Button outline={true} color={'warning'}>
                <Link href={`/auctions/update/${id}`}>Update auction</Link>
            </Button>
        </div>
    );
}

export default EditButton;