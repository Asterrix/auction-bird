import axios from "axios";
import {environment} from "../../environments/environment.ts";

type Bid = {
  clientId: string;
  itemId: string;
  bidAmount: number;
}

const biddingService = {
  async makeBid(form: Bid) {
    const response = await axios.postForm(`${environment.apiUrl}/bids`, form, {
      withCredentials: true,
      headers: {
        "Access-Control-Allow-Origin": environment.apiUrl,
        "Access-Control-Allow-Credentials": "true",
      }
    });

    return response.data;
  },

  async findHighestBidder(itemId: string) {
    const response = await axios.get(`${environment.apiUrl}/bids/${itemId}`);
    return response.data;
  }
};

export type {Bid};
export {biddingService};