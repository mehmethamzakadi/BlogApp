import { Box, Center, Flex } from "@chakra-ui/react";
import { ReactNode } from "react";
import Profil from "../../Features/Profil/Profil";
import Footer from "../Footer/Footer";
import Navbar from "../Navbar/Navbar";

interface Props {
  children?: ReactNode;
}

const MainLayout = ({ children }: Props) => {
  return (
    <>
      <Navbar />
      <Flex>
        <Box flex="1">
          <Profil />
        </Box>
        <Center flex="3">
          <main>{children}</main>
        </Center>
      </Flex>
      <Footer />
    </>
  );
};

export default MainLayout;
