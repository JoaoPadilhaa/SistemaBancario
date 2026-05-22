import { createContext, useContext, useState } from "react";

const UserContext = createContext<any>(null);

export function UserProvider({ children }: {children: React.ReactNode}){
    // Inicializa direto do localStorage — sem useEffect, sem delay
    const [usuario, setUsuario] = useState<any>(() => {
        const dados = localStorage.getItem("usuario");
        return dados ? JSON.parse(dados) : null;
    });

    return(
        <UserContext.Provider value={{ usuario, setUsuario}}>
            {children}
        </UserContext.Provider>
    )
}

export function useUsuario(){
    return useContext(UserContext);
}
