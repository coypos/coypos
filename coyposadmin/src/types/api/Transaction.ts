import { UserModel } from "@/types/api/User";
import { ProductModel } from "@/types/api/Product";

export interface TransactionModel {
  id: number;

  createdDate: Date | string;

  updatedDate: Date | string;

  userID: number;

  user: UserModel;

  productID: number;
  product: ProductModel;

  quantity: number;

  totalPrice: number;

  transactionTime: Date | string;
}
