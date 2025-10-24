import { useEffect, useRef } from 'react';
import { Bold, Italic, List, ListOrdered, Underline, X } from 'lucide-react';

import { Button } from '../ui/button';

interface RichTextEditorProps {
  value: string;
  onChange: (value: string) => void;
  placeholder?: string;
}

export function RichTextEditor({ value, onChange, placeholder }: RichTextEditorProps) {
  const editorRef = useRef<HTMLDivElement | null>(null);

  useEffect(() => {
    if (!editorRef.current) {
      return;
    }

    if (editorRef.current.innerHTML !== value) {
      editorRef.current.innerHTML = value;
    }
  }, [value]);

  const handleInput = () => {
    const nextValue = editorRef.current?.innerHTML ?? '';
    onChange(nextValue);
  };

  const applyCommand = (command: string) => {
    if (!editorRef.current) {
      return;
    }

    editorRef.current.focus();
    document.execCommand(command);
    handleInput();
  };

  const clearFormatting = () => {
    if (!editorRef.current) {
      return;
    }

    editorRef.current.focus();
    document.execCommand('removeFormat');
    document.execCommand('unlink');
    handleInput();
  };

  return (
    <div className="rounded-md border bg-background">
      <div className="flex items-center gap-1 border-b bg-muted/30 px-2 py-1">
        <Button type="button" variant="ghost" size="icon" onClick={() => applyCommand('bold')}>
          <Bold className="h-4 w-4" />
          <span className="sr-only">Kalın</span>
        </Button>
        <Button type="button" variant="ghost" size="icon" onClick={() => applyCommand('italic')}>
          <Italic className="h-4 w-4" />
          <span className="sr-only">İtalik</span>
        </Button>
        <Button type="button" variant="ghost" size="icon" onClick={() => applyCommand('underline')}>
          <Underline className="h-4 w-4" />
          <span className="sr-only">Altı Çizili</span>
        </Button>
        <Button type="button" variant="ghost" size="icon" onClick={() => applyCommand('insertUnorderedList')}>
          <List className="h-4 w-4" />
          <span className="sr-only">Sırasız Liste</span>
        </Button>
        <Button type="button" variant="ghost" size="icon" onClick={() => applyCommand('insertOrderedList')}>
          <ListOrdered className="h-4 w-4" />
          <span className="sr-only">Sıralı Liste</span>
        </Button>
        <Button type="button" variant="ghost" size="icon" onClick={clearFormatting}>
          <X className="h-4 w-4" />
          <span className="sr-only">Biçimlendirmeyi Temizle</span>
        </Button>
      </div>
      <div className="relative">
        {!value && placeholder ? (
          <span className="pointer-events-none absolute left-3 top-2 text-sm text-muted-foreground">
            {placeholder}
          </span>
        ) : null}
        <div
          ref={editorRef}
          className="min-h-[200px] whitespace-pre-wrap px-3 py-2 text-sm focus:outline-none"
          contentEditable
          onInput={handleInput}
          role="textbox"
          aria-multiline="true"
          suppressContentEditableWarning
        />
      </div>
    </div>
  );
}
