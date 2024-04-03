import { useContext } from 'react';
import { decodeToken } from '../util/tokenUtil';
import { AuthContext } from 'src/contexts/AuthContext';

interface IAuth {
  IsAuthenticated: string;
  Name: string;
  Email: string;
  Roles?: string[];
}
interface IAuthProps {
  roles?: string[];
}
const useAuth = (props?: IAuthProps) => {
  var { token } = useContext(AuthContext);
  var auth = decodeToken<IAuth>(token);
  var isAuthenticated = false;
  var isAllow = true;
  if (auth) {
    isAuthenticated = auth.IsAuthenticated === 'true';
    if (isAuthenticated && props && props.roles) {
      isAllow = props.roles.some((role) => auth?.Roles?.includes(role));
    }
  }
  return { isAuthenticated, isAllow };
};
export default useAuth;
