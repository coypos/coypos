import { UserModel } from "@/types/api/User";
import { ProductModel } from "@/types/api/Product";

export interface TransactionModel {
  ID: number;

  CreatedDate: Date | string;

  UpdatedDate: Date | string;

  UserID: number;

  User: UserModel;

  ProductID: number;

  Product: ProductModel;

  Quantity: number;

  TotalPrice: number;

  TransactionTime: Date | string;
}
