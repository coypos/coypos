import { CategoryModel } from "@/types/api/Category";

export interface ProductModel {
  id: number | null;

  createDate: Date | string | null;

  updateDate: Date | string | null;

  enabled: boolean | null;

  name: string;

  barcode: string | null;

  price: number;

  isLoose: boolean | null;

  weight: number | null;

  description: string | null;

  category: CategoryModel | null;

  image: string | null;
}
