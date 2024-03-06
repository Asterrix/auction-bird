import axios from "axios";
import {Pageable} from "../../utils/types/pagination/pageable.type.ts";
import {ItemFilter} from "./item.filter.ts";
import {environment} from "../../environments/environment.ts";

interface ItemSummary {
  id: string;
  name: string;
  initialPrice: number;
  mainImage: {
    id: number;
    imageUrl: string;
  };
}

interface ItemInfo {
  id: string;
  name: string;
  description: string;
  currentPrice: number;
  timeLeft: string;
  isActive: boolean;
  images: {
    id: number;
    imageUrl: string;
  }[];
}

type CreateItem = {
  name: string;
  category: string;
  subcategory: string;
  description: string;
  initialPrice: number;
  startTime: Date;
  endTime: Date;
  images: File[];
}

interface Param {
  page: number;
  size: number;
  search?: string;
  categories?: string;
}

export const itemService = {
  async getItems(pageable: Pageable, filter?: Partial<ItemFilter>) {
    const params: Param = {
      page: pageable.page,
      size: pageable.size,
      search: filter?.search || undefined,
      categories: filter?.categories?.flat().join(",") || undefined
    };

    const response = await axios.get(`${environment.apiUrl}/items`, {params: params});
    return response.data;
  },

  async getItem(id: string) {
    const response = await axios.get(`${environment.apiUrl}/items/${id}`);
    return response.data;
  },
  
  async createItem(item: CreateItem){
    const formData = new FormData();
    formData.append("name", item.name);
    formData.append("category", item.category);
    formData.append("subcategory", item.subcategory);
    formData.append("description", item.description);
    formData.append("initialPrice", item.initialPrice.toString());
    formData.append("startTime", item.startTime.toISOString());
    formData.append("endTime", item.endTime.toISOString());
    item.images.forEach((image) => formData.append("images", image));
    
    const response = await axios.postForm(`${environment.apiUrl}/items`, formData, {
      withCredentials: true,
      headers: {
        "Access-Control-Allow-Origin": environment.apiUrl,
        "Access-Control-Allow-Credentials": "true",
        "Content-Type": "multipart/form-data",
      },
    });
    return response.data;
  }
};

export type {ItemSummary, ItemInfo, CreateItem};