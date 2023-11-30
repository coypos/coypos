import webpack from "webpack";
new webpack.DefinePlugin({
  __VUE_OPTIONS_API__: true,
});
