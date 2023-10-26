import { UserModel } from "@/types/api/User";

export interface TransactionLog {
  ID: number;

  CreatedDate: Date | string;

  UpdatedDate: Date | string;

  UserID: number;

  User: UserModel;

  Action: string;

  Timestamp: Date | string;
}
