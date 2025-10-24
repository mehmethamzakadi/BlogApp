export function sanitizeHtml(html: string): string {
  if (!html) {
    return '';
  }

  if (typeof window === 'undefined') {
    return html;
  }

  const parser = new window.DOMParser();
  const documentFragment = parser.parseFromString(html, 'text/html');
  const { body } = documentFragment;

  if (!body) {
    return '';
  }

  const blockedTags = ['script', 'style', 'iframe', 'object', 'embed', 'link', 'meta'];
  blockedTags.forEach((tag) => {
    body.querySelectorAll(tag).forEach((element) => element.remove());
  });

  const walker = documentFragment.createTreeWalker(body, NodeFilter.SHOW_ELEMENT);
  const attributeProtocols = ['href', 'src', 'xlink:href'];

  const isUnsafeProtocol = (value: string) => {
    const normalised = value.trim().toLowerCase();
    return normalised.startsWith('javascript:') || normalised.startsWith('vbscript:') || normalised.startsWith('data:');
  };

  while (walker.nextNode()) {
    const element = walker.currentNode as HTMLElement;
    const attributes = Array.from(element.attributes);

    attributes.forEach((attribute) => {
      const name = attribute.name.toLowerCase();

      if (name.startsWith('on')) {
        element.removeAttribute(attribute.name);
        return;
      }

      if (name === 'style') {
        element.removeAttribute(attribute.name);
        return;
      }

      if (attributeProtocols.includes(name) && isUnsafeProtocol(attribute.value)) {
        element.removeAttribute(attribute.name);
      }
    });
  }

  return body.innerHTML;
}
