export function Footer() {
  return (
    <footer className="border-t bg-background">
      <div className="container flex flex-col items-center justify-between gap-2 py-6 text-sm text-muted-foreground sm:flex-row">
        <span>&copy; {new Date().getFullYear()} BlogApp. Tüm hakları saklıdır.</span>
        <span>Modern içerik platformu</span>
      </div>
    </footer>
  );
}
