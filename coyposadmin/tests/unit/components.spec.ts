import { expect } from "chai";
import { shallowMount, mount, config } from "@vue/test-utils";
import categoriesComponent from "@/components/CategoriesComponent.vue";
describe("categoriesComponent.vue", () => {
  it("should render", () => {
    const category = { name: "testowakategoria" };

    const wrapper = shallowMount(categoriesComponent, {
      props: { category: category },
    });
    expect(wrapper.find(".backbutton").isVisible()).to.be.true;
    expect(wrapper.find(".backbutton").find("img").isVisible()).to.be.true;
  });
});
