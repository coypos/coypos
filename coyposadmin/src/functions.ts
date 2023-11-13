import { Modal } from "bootstrap";

export function showModal() {
  const myModal = new Modal(
    document.getElementById("staticBackdrop") as Element
  );
  myModal.show();
}
export function showDeleteModal() {
  const myModal = new Modal(document.getElementById("deleteModal") as Element);
  myModal.show();
}
export async function compressImage(
  base64: string,
  maxSizeInBytes: number,
  maxWidth: number
): Promise<string> {
  const img = new Image();
  img.src = base64;
  await img.decode();
  let width = img.width;
  let height = img.height;
  if (width > maxWidth) {
    height = (maxWidth / width) * height;
    width = maxWidth;
  }
  let quality = 1;
  let base64Image = base64;
  while (base64Image.length > maxSizeInBytes && quality > 0.1) {
    const canvas = document.createElement("canvas");
    canvas.width = width;
    canvas.height = height;
    const ctx = canvas.getContext("2d");
    ctx?.drawImage(img, 0, 0, width, height);
    base64Image = canvas.toDataURL("image/jpeg", quality);
    quality -= 0.1;
  }
  return base64Image.substring("data:image/jpeg;base64,".length);
}
