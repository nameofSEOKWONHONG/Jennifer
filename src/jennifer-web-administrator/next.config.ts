import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  /* config options here */
  domain: process.env.NEXT_PUBLIC_API_DOMAIN!

};

export default nextConfig;
