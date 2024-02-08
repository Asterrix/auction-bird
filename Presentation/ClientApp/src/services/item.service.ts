import {Pageable} from "../utils/types/pagination/pageable.type.ts";
import axios from "axios";
import {environment} from "../environments/environments.development.ts";

interface ItemSummary {
  id: string;
  name: string;
  initialPrice: number;
  mainImage: {
    id: number;
    imageUrl: string;
  };
}

export const itemService = {
  async getItems(pageable: Pageable) {
    const response = await axios.get(`${environment.apiUrl}/items`, {
      params: pageable
    });
    return response.data;
  }
};

export type {ItemSummary};