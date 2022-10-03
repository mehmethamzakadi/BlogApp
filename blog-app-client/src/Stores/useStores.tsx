import { useContext } from "react";
import { rootStoreContext } from ".";

export const useStores = () => useContext(rootStoreContext);
