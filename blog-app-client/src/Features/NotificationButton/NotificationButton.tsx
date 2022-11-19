import { useState } from "react";
import { FaEnvelope } from "react-icons/fa";

import { IconButton, useColorModeValue } from "@chakra-ui/react";

const NotificationButton = () => {
  const [show, setShow] = useState<boolean>(false);
  const EnvelopeIcon = useColorModeValue(FaEnvelope, FaEnvelope);
  return (
    <IconButton
      size="md"
      fontSize="lg"
      variant="ghost"
      color="current"
      onClick={() => setShow(!show)}
      icon={<EnvelopeIcon />}
      aria-label={`Envelope`}
    />
  );
};

export default NotificationButton;
