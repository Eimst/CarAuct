'use client'

import React from 'react';
import {Dropdown, DropdownDivider, DropdownItem} from "flowbite-react";
import Link from "next/link";
import {User} from "next-auth";
import {usePathname, useRouter} from "next/navigation";
import {HiCog, HiUser} from "react-icons/hi2";
import {AiFillCar, AiFillTrophy, AiOutlineLogout} from "react-icons/ai";
import {signOut} from "next-auth/react";
import {useParamStore} from "@/hooks/useParamsStore";



type Props = {
    user: User;
}

const UserActions = ({user}: Props) => {
    const router = useRouter();
    const pathName = usePathname();

    const setParams = useParamStore(state => state.setParams)

    const setWinner = () => {
        setParams({winner: user.username, seller: undefined})

        if (pathName !== '/') {
            router.push("/")
        }
    }

    const setSeller = () => {
        setParams({seller: user.username, winner: undefined})

        if (pathName !== '/') {
            router.push("/")
        }
    }

    return (
        <div>
            <Dropdown inline label={`Welcome ${user.name}`}>
                <DropdownItem icon={HiUser} onClick={() => setSeller()}>
                    My Auctions
                </DropdownItem>

                <DropdownItem icon={AiFillTrophy} onClick={() => setWinner()}>
                    <Link href={`/`}>
                        Auctions won
                    </Link>
                </DropdownItem>

                <DropdownItem icon={AiFillCar}>
                    <Link href={`/auctions/create`}>
                        Sell my car
                    </Link>
                </DropdownItem>

                <DropdownItem icon={HiCog}>
                    <Link href={`/session`}>
                        Session (dev)
                    </Link>
                </DropdownItem>

                <DropdownDivider/>

                <DropdownItem icon={AiOutlineLogout} onClick={() => signOut({redirectTo: '/'})}>
                    Sign out
                </DropdownItem>
            </Dropdown>
        </div>
    );
};

export default UserActions;