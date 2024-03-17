import axios from "axios";
import {environment} from "../../environments/environment.ts";

export const paymentService = {
  async createPaymentSession(username: string, itemId: string){
    const response = await axios.postForm(`${environment.apiUrl}/payments`, {username, itemId});
    return response.data;
  }
}