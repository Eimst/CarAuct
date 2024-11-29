'use client'

import React from 'react';
import {Button} from "flowbite-react";
import {signIn} from "next-auth/react";

const LoginButton = () => {
    return (
        <Button className={`rounded-2xl border-2 border-red-400 focus:ring-1 focus:ring-red-400`}
                outline onClick={() =>
            signIn('id-server', {redirect: true, redirectTo: '/'}, {prompt: 'login'})}>Login</Button>
    );
};

export default LoginButton;