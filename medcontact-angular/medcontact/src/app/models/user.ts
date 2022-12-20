
export class User {
  id?: string;
  accessToken?: string;
  refreshToken?: string;
  tokenExpiration?: Date;
  roles?: Array<string>;
}
