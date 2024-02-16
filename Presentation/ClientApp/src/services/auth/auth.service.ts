import axios from "axios";
import {environment} from "../../environments/environment.ts";

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
  async signUp(form: SignUp) {
    const response = await axios.postForm(`${environment.apiUrl}/signup`, form);
    return response.data;
  }
};

export {authService};
export type {SignUp};