import { Box, SimpleGrid } from "@chakra-ui/react";
import React from "react";
import Card from "./Card";

const CardItems = () => {
  return (
    <>
      <SimpleGrid columns={[1, null, 3]} spacing="10px">
        <Box>
          <Card />
        </Box>
        <Box>
          <Card />
        </Box>
        <Box>
          <Card />
        </Box>
        <Box>
          <Card />
        </Box>
        <Box>
          <Card />
        </Box>
        <Box>
          <Card />
        </Box>
      </SimpleGrid>
    </>
  );
};

export default CardItems;
