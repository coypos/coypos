import { ProductModel } from "@/types/api/Product";

export interface PromotionModel {
  id: number | null;

  ids: string | null;

  discountPercentage: number | null;

  startDate: Date | string | null;

  endDate: Date | string | null;

  createDate: Date | string | null;

  updateDate: Date | string | null;
  affectedProducts: ProductModel[] | null;
}
