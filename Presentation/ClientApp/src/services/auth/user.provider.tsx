import {jwtDecode, JwtPayload} from "jwt-decode";
import React, {createContext, useCallback, useEffect, useState} from "react";
import Cookies from "js-cookie";
import {apiService} from "../api.service.ts";
import {SignOut} from "./auth.service.ts";

type User = {
  present: boolean;
  fullName: string;
  email: string;
  username: string;
  subject: string;
}

interface UserToken extends JwtPayload {
  subject: string;
  given_name: string;
  family_name: string;
  email: string;
}

interface UserContext {
  user: User | null;
  updateUser: () => void;
  signOut: () => void;
}

const userContext = createContext<UserContext>({
  user: null,
  updateUser: () => {
  },
  signOut: () => {
  },
});

const UserProvider: React.FC<{ children: React.ReactNode }> = ({children}) => {
  const [user, setUser] = useState<User | null>(null);

  const updateUser = useCallback(() => {
    const token: string | undefined = Cookies.get("idToken");
    if (!token) return;

    let jwt: UserToken;

    try {
      jwt = jwtDecode(token);
    } catch (error) {
      return;
    }

    // Extract the first and last name from the JWT
    const firstName: string = jwt.given_name;
    const lastName: string = jwt.family_name;
    const fullName: string = `${firstName} ${lastName}`;

    // Extract the email from the JWT
    const email: string = jwt.email;

    // Extract the username from the JWT
    const username: string = jwt["cognito:username"];
    
    // Extract the subject from the JWT
    const subject: string = jwt.sub;

    setUser({
      present: true,
      fullName: fullName,
      email: email,
      username: username,
      subject: subject,
    });
  }, []);

  const signOut = () => {
    if (!user) return;

    const form: SignOut = {
      username: user.username,
    };

    apiService.authentication.signOut(form)
      .then(() => {
        setUser(null);
        window.location.href = "/";
      })
      .catch(() => {
        Cookies.remove("idToken");
        window.location.href = "/";
      });
  };

  useEffect(() => {
    updateUser();
  }, [updateUser]);

  return (
    <userContext.Provider value={{user, updateUser, signOut}}>
      {children}
    </userContext.Provider>
  );
};

export {userContext, UserProvider};