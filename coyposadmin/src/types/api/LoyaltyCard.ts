import { UserModel } from "@/types/api/User";

export interface LoyaltyCardModel {
  ID: number;

  CreatedDate: Date | string;

  UpdatedDate: Date | string;

  UserID: number;

  User: UserModel;

  CardNumber: string;

  Points: number;
}
