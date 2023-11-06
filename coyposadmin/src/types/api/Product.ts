import { CategoryModel } from "@/types/api/Category";

export interface ProductModel {
  id: number | null;

  createDate: Date | string | null;

  updateDate: Date | string | null;

  enabled: boolean | null;

  name: string | null;

  barcode: string | null;

  price: number | null;

  isLoose: boolean | null;

  weight: number | null;

  description: string | null;

  category: CategoryModel | null;

  image: string | null;
}
