import axios from "axios";
import {environment} from "../../environments/environment.ts";

type SignIn = {
  clientId: string;
  email: string;
  password: string;
}

type SignUp = {
  clientId: string;
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  termsAndConditions: boolean;
  isOver18: boolean;
}

const authService = {
  async signIn(form: SignIn) {
    const response = await axios.postForm(`${environment.apiUrl}/signin`, form, {
      withCredentials: true,
      headers: {
        "Access-Control-Allow-Origin": environment.apiUrl,
        "Access-Control-Allow-Credentials": "true",
      }
    });
    return response.data;
  },
  
  async signUp(form: SignUp) {
    const response = await axios.postForm(`${environment.apiUrl}/signup`, form);
    return response.data;
  }
};

export {authService};
export type {SignIn, SignUp};