import { Daytimetabledto } from "./daytimetabledto";
import { Meddatadto } from "./meddatadto";
import { Specialitydto } from "./specialitydto";
import { Userdto } from "./userdto";

export class Doctordatadto {
  id?: string;
  roleId?: string;
  userId?: string;
  user?: Userdto;
  isBlocked?: boolean;
  forDeletion?: boolean;
  specNameReserved?: string;
  specialityId?: string;
  speciality?:  Specialitydto;
  dayTimeTables?: Array<Daytimetabledto>;
  medData?: Array<Meddatadto>;
}
