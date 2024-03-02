import {Pageable} from "../../utils/types/pagination/pageable.type.ts";
import axios from "axios";
import {environment} from "../../environments/environment.ts";

type ActiveItem = {
  id: string;
  name: string;
  timeLeft: string;
  initialPrice: number;
  numberOfBids: number;
  highestBid: number;
  mainImage: {
    id: string;
    imageUrl: string;
  };
}

type SoldItem = {
  id: string;
  name: string;
  initialPrice: number;
  numberOfBids: number;
  finalPrice: number;
  mainImage: {
    id: string;
    imageUrl: string;
  };
}

const userService = {
  async listActiveItems(userId: string, pageable: Pageable) {
    const response = await axios.get(`${environment.apiUrl}/users/${userId}/items`, {
      params: {
        page: pageable.page,
        size: pageable.size
      }
    });
    return response.data;
  },

  async listSoldItems(userId: string, pageable: Pageable) {
    const response = await axios.get(`${environment.apiUrl}/users/${userId}/sold-items`, {
      params: {
        page: pageable.page,
        size: pageable.size
      }
    });
    return response.data;
  }
};

export {userService};
export type {ActiveItem, SoldItem};