import { SignIn } from "@/types/auth/signin";
import { TokenResponse } from "@/types/auth/tokenResponse";
import { BaseResponse } from "@/types/common/baseResponse";
import axios, { AxiosError } from "axios";

export async function login(params: SignIn) {
  try {
    const result = await axios.post<BaseResponse<TokenResponse>>(
      "https://localhost:5001/api/auth/login",
      params
    );

    if (!result.data.success) {
      return { error: result.data.success, message: result.data.message };
    }

    localStorage.setItem("jwt", JSON.stringify(result.data.data.token));

    return result.data;
  } catch (e) {
    const error = e as AxiosError;
    return {
      error,
    };
  }
}
