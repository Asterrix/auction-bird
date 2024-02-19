import {jwtDecode, JwtPayload} from "jwt-decode";
import React, {createContext, useCallback, useEffect, useState} from "react";
import {useCookies} from "react-cookie";

type User = {
  present: boolean;
  fullName: string;
  email: string;
}

interface UserToken extends JwtPayload {
  given_name: string;
  family_name: string;
  email: string;
}

interface UserContext {
  user: User | null;
  updateUser: () => void;
}

const userContext = createContext<UserContext>({
  user: null,
  updateUser: () => {
  },
});

const UserProvider: React.FC<{ children: React.ReactNode }> = ({children}) => {
  const [user, setUser] = useState<User | null>(null);
  const [cookies] = useCookies(["idToken"]);

  const updateUser = useCallback(() => {
    if (cookies.idToken) {
      const jwt: UserToken = jwtDecode(cookies.idToken);
      
      // Extract the first and last name from the JWT
      const firstName: string = jwt.given_name;
      const lastName: string = jwt.family_name;
      const fullName: string = `${firstName} ${lastName}`;
      
      // Extract the email from the JWT
      const email: string = jwt.email;

      setUser({
        present: true,
        fullName: fullName,
        email: email
      });
    }
  }, [cookies.idToken]);

  useEffect(() => {
    updateUser();
  }, [updateUser]);

  return (
    <userContext.Provider value={{user, updateUser}}>
      {children}
    </userContext.Provider>
  );
};

export {userContext, UserProvider};