import { Acsdatadto } from "./acsdatadto";
import { Doctordatadto } from "./doctordatadto";
import { Filedatadto } from "./filedatadto";
import { Roledto } from "./roledto";

export class Userdto {
  id?: string;
  username?: string;
  email?: string;
  passwordHash?: string;
  phoneNumber?: string;
  name?: string;
  surname?: string;
  midName?: string;
  birthDate?: Date;
  gender?: string;
  address?: string;
  familyId?: string;
  customerDataId?: string;
  isOnline?: boolean;
  isDependent?: boolean;
  isFullBlocked?: boolean;
  fileDatas?: Array<Filedatadto>;
  doctorData?: Array<Doctordatadto>;
  acsDatas?: Array<Acsdatadto>;
  roles?: Array<Roledto>;
  registrationDate?: Date;
  lastLogin?: Date;

  accessToken?: string;
  refreshToken?: string;
  tokenExpiration?: Date;
}
