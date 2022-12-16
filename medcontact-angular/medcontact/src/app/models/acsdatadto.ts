import { Extradatadto } from "./extradatadto";
import { Roledto } from "./roledto";
import { Userdto } from "./userdto";

export class Acsdatadto {
  id?: string;
  roleId?: string;
  role?: Roledto;
  users?: Array<Userdto>;
  isBlocked?: boolean;
  extraDatas?: Array<Extradatadto>;
}
