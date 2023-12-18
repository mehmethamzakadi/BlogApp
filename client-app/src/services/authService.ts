import { SignIn } from "@/types/auth/signin";
import { TokenResponse } from "@/types/auth/tokenResponse";
import { BaseResponse } from "@/types/common/baseResponse";
import axios, { AxiosError } from "axios";

export async function login(params: SignIn) {
  debugger;
  try {
    const result = await axios.post<BaseResponse<TokenResponse>>(
      "https://localhost:7285/api/auth/login",
      params
    );
    localStorage.setItem("jwt", JSON.stringify(result.data));

    return result.data;
  } catch (e) {
    const error = e as AxiosError;
    return {
      error,
    };
  }
}
