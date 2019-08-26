const path = require('path');
module.exports = {
  mode: "development",
  entry: './src/Authoring.ts', 
  module: {
    rules: [
      {
        test: /\.tsx?$/,
        use: 'ts-loader',
        exclude: /node_modules/
      },      
    ]
  },
  devServer:
  {    
    compress: true,
    port: 9000
  },
  output: {
    filename: 'powerappsauthoringsdksample.js',
    libraryTarget: "umd",
    path: path.resolve(__dirname, 'dist')
  }
}