import { Appointmentdto } from "./appointmentdto";
import { Meddatadto } from "./meddatadto";
import { Userdto } from "./userdto";

export class Customerdatadto {
   id?: string;
   roleId?: string;
   userId?: string;
   user?: Userdto;
   isBlocked?: boolean;
   appointments?: Array<Appointmentdto>;
   medData?: Array<Meddatadto>;
}
