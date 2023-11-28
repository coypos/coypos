export interface UserModel {
  id: number | null;

  createDate: Date | string | null;

  updateDate: Date | string | null;

  name: string | null;

  role: string | null;

  cardNumber: string | null;

  phoneNumber: string | null;

  points: number | null;

  email: string | null;

  password: string | null;

  salt: string | null;

  loginToken: string | null;

  loginTokenValidDate: Date | string | null;
}
