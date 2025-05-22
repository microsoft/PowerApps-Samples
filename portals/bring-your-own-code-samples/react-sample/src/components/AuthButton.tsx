import { IconButton, Tooltip } from '@mui/material';
import {
    Login,
    Logout
} from '@mui/icons-material';
import React from 'react';
export const AuthButton = () => {
    const username = (window as any)["Microsoft"]?.Dynamic365?.Portal?.User?.userName ?? "";
    const firstName = (window as any)["Microsoft"]?.Dynamic365?.Portal?.User?.firstName ?? "";
    const lastName = (window as any)["Microsoft"]?.Dynamic365?.Portal?.User?.lastName ?? "";
    const isAuthenticated = username !== "";
    const [token, setToken] = React.useState<string>("");
    
    // @ts-ignore
    const tenantId = import.meta.env.VITE_TENANT_ID;

    React.useEffect(() => {
        const getToken = async () => {
            try {
                const token = await (window as any).shell.getTokenDeferred();
                setToken(token);
            } catch (error) {
                console.error('Error fetching token:', error);
            }
        };
        getToken();
    }, []);

    return (
        <div className="flex items-center gap-4">
            {isAuthenticated ? (
                <>
                    <span className="text-sm">Welcome {firstName + " " + lastName}</span>
                    <Tooltip title="Logout">
                        <IconButton color="primary" onClick={() => window.location.href = "/Account/Login/LogOff?returnUrl=%2F"}>
                            <Logout />
                        </IconButton>
                    </Tooltip>
                </>
            ) : (
                <form action="/Account/Login/ExternalLogin" method="post">
                    <input name="__RequestVerificationToken" type="hidden" value={token} />
                    <Tooltip title="Login">
                        <IconButton name="provider" type="submit" color="primary" value={`https://login.windows.net/${tenantId}/`}>
                            <Login />
                        </IconButton>
                    </Tooltip>
                </form>
            )}
        </div>
    );
};
