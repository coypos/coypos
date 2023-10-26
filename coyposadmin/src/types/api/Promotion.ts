import { ProductModel } from "@/types/api/Product";

export interface PromotionModel {
  ID: number;

  CreatedDate: Date | string;

  UpdatedDate: Date | string;

  ProductID: number;

  Product: ProductModel;

  DiscountPercentage: number;

  StartDate: Date | string;

  EndDate: Date | string;
}
