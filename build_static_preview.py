import os
import re
import urllib.request

BASE_URL = "http://localhost:5299"
REPO_SUBPATH = "/gateway-centre"  # GitHub Pages project-site base path
OUT_DIR = os.path.join(
    r"C:\Users\OsamaArshadAhmad\source\repos\GatewayCentre", "docs"
)

ROUTES = [
    "/",
    "/services",
    "/our-spaces",
    "/packages",
    "/gallery",
    "/about",
    "/contact",
    "/request-a-quote",
]

# Known internal page routes -> where they should point after prefixing (with trailing slash
# so GitHub Pages resolves them to the matching directory's index.html).
PAGE_ROUTE_FIXUPS = {
    REPO_SUBPATH: REPO_SUBPATH + "/",
    REPO_SUBPATH + "/services": REPO_SUBPATH + "/services/",
    REPO_SUBPATH + "/our-spaces": REPO_SUBPATH + "/our-spaces/",
    REPO_SUBPATH + "/packages": REPO_SUBPATH + "/packages/",
    REPO_SUBPATH + "/gallery": REPO_SUBPATH + "/gallery/",
    REPO_SUBPATH + "/about": REPO_SUBPATH + "/about/",
    REPO_SUBPATH + "/contact": REPO_SUBPATH + "/contact/",
    REPO_SUBPATH + "/request-a-quote": REPO_SUBPATH + "/request-a-quote/",
}


def fetch(path: str) -> str:
    with urllib.request.urlopen(BASE_URL + path) as resp:
        return resp.read().decode("utf-8")


def fetch_binary(path: str) -> bytes:
    with urllib.request.urlopen(BASE_URL + path) as resp:
        return resp.read()


LOCAL_REF_RE = re.compile(r'(href|src)="(/(?!/)[^"]*|[^":#/][^"]*\.(?:css|js))"')


def collect_local_refs(html: str) -> set[str]:
    refs = set()
    for _, value in LOCAL_REF_RE.findall(html):
        if value.startswith(("http://", "https://", "mailto:", "tel:")):
            continue
        refs.add(value)
    return refs


def is_page_route(value: str) -> bool:
    path = value.split("?", 1)[0]
    return path in ("/", "/services", "/our-spaces", "/packages", "/gallery", "/about", "/contact", "/request-a-quote")


CSS_URL_RE = re.compile(r'url\((["\']?)(/(?!/)[^)\'"]+)\1\)')


def download_asset(ref: str, downloaded: set[str]) -> None:
    if ref in downloaded:
        return
    downloaded.add(ref)

    local_path = os.path.join(OUT_DIR, ref.lstrip("/"))
    os.makedirs(os.path.dirname(local_path), exist_ok=True)

    data = fetch_binary(ref if ref.startswith("/") else "/" + ref)
    with open(local_path, "wb") as f:
        f.write(data)

    if local_path.endswith(".css"):
        text = data.decode("utf-8")
        for _, css_ref in CSS_URL_RE.findall(text):
            download_asset(css_ref, downloaded)
        # Rewrite root-relative url(...) references to the GitHub Pages subpath
        text = CSS_URL_RE.sub(
            lambda m: f'url({m.group(1)}{REPO_SUBPATH}{m.group(2)}{m.group(1)})', text
        )
        with open(local_path, "w", encoding="utf-8") as f:
            f.write(text)


def rewrite_html(html: str) -> str:
    # No separate <base> tag handling needed: its href="/" is matched and correctly
    # prefixed to "/gateway-centre/" by the same generic pass below. Doing it as a
    # second, separate replace caused the base tag to be prefixed twice.

    def repl(m: re.Match) -> str:
        attr, value = m.group(1), m.group(2)
        if value.startswith(("http://", "https://", "mailto:", "tel:")):
            return m.group(0)
        if value.startswith("/"):
            return f'{attr}="{REPO_SUBPATH}{value}"'
        return m.group(0)

    html = LOCAL_REF_RE.sub(repl, html)

    for old, new in PAGE_ROUTE_FIXUPS.items():
        html = html.replace(f'"{old}"', f'"{new}"')
        html = html.replace(f'"{old}?', f'"{new}?')

    # Strip the Blazor Server bootstrap script — there's no server behind this static
    # preview, so leaving it in just causes failed SignalR connection attempts.
    html = re.sub(r'\s*<script src="[^"]*_framework/blazor\.web[^"]*"></script>', "", html)

    return html


def main() -> None:
    os.makedirs(OUT_DIR, exist_ok=True)
    downloaded: set[str] = set()

    for route in ROUTES:
        html = fetch(route)
        for ref in collect_local_refs(html):
            if not is_page_route(ref):
                download_asset(ref, downloaded)

        html = rewrite_html(html)

        if route == "/":
            out_path = os.path.join(OUT_DIR, "index.html")
        else:
            out_path = os.path.join(OUT_DIR, route.strip("/"), "index.html")
        os.makedirs(os.path.dirname(out_path), exist_ok=True)
        with open(out_path, "w", encoding="utf-8") as f:
            f.write(html)
        print(f"wrote {out_path}")

    with open(os.path.join(OUT_DIR, ".nojekyll"), "w") as f:
        pass

    print(f"\nDownloaded {len(downloaded)} local assets.")


if __name__ == "__main__":
    main()
