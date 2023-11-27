import { UserModel } from "@/types/api/User";

export interface LoyaltyCardModel {
  id: number;

  createdDate: Date | string;

  updatedDate: Date | string;

  userID: number;

  user: UserModel;

  cardNumber: string;

  points: number;
}
